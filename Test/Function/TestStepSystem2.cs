using System;
using System.Collections.Generic;
using System.IO;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    /// <summary>
    /// TezStepSystem2 的自动化测试(替代原来的交互式 TestStepSystem2)。
    ///
    /// 设计:
    ///   - 每个用例独立创建一个 TezStepSystem2,不共享静态状态
    ///   - 不依赖按键,整段跑完给出汇总;有失败时抛异常
    ///   - 框架 update() 里的调试输出在推进帧时被吞掉,避免污染测试结果
    ///   - 断言的是"行为契约"(执行顺序、挂起/恢复、结束),不是内部实现细节
    ///
    /// 用法:整文件替换 Test/Function/TestStepSystem2.cs,
    /// 类名与基类保持不变,所以 TestProgram 不用改。
    /// </summary>
    class TestStepSystem2 : TezBaseTest
    {
        public TestStepSystem2() : base("StepSystem2")
        {

        }

        public override void init()
        {

        }

        public override void run()
        {
            sPassed = 0;
            sFailed = 0;
            sFailures.Clear();

            this.testSequentialExecution();
            this.testEmptyGroup();
            this.testWaitAndResume();
            this.testAbort();
            this.testWaitChildGroup();
            this.testTwoWaitsOnSameGroup();
            this.testNestedWait();
            this.testSequenceExecutorOrder();
            this.testSequenceExecutorWaitInSecondGroup();
            this.testSequenceExecutorAbortAdvances();
            this.testParentEndsExecutor();
            this.testRunImmediatelyVsNextFrame();

            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine($"步骤系统测试:通过 {sPassed} 项,失败 {sFailed} 项");

            if (sFailed > 0)
            {
                for (int i = 0; i < sFailures.Count; i++)
                {
                    Console.WriteLine("  失败: " + sFailures[i]);
                }

                throw new Exception($"TezStepSystem2 测试失败 {sFailed} 项");
            }
        }

        protected override void onClose()
        {

        }

        #region 用例

        /// <summary>
        /// 一个组里的步骤按顺序执行,每帧一步,最后系统清空。
        /// </summary>
        private void testSequentialExecution()
        {
            Console.WriteLine();
            Console.WriteLine("-- 顺序执行与帧推进");

            var sys = new TezStepSystem2();
            var order = new List<string>();

            var group = new TezStepGroup();
            group.addStep(m => order.Add("s1"));
            group.addStep(m => order.Add("s2"));
            group.addStep(m => order.Add("s3"));

            sys.runInNextFrame<TezStepStackExecutor>(group);

            CheckOrder(order, "runInNextFrame 不在当帧执行");

            Tick(sys);
            CheckOrder(order, "第 1 帧执行第 1 步", "s1");
            Check(!sys.isEmpty(), "第 1 帧后系统仍在运行");

            Tick(sys);
            CheckOrder(order, "第 2 帧执行第 2 步", "s1", "s2");

            Tick(sys);
            CheckOrder(order, "第 3 帧执行第 3 步", "s1", "s2", "s3");

            Tick(sys);
            Check(sys.isEmpty(), "全部执行完后系统清空");
            CheckOrder(order, "每个步骤只执行一次", "s1", "s2", "s3");
        }

        /// <summary>
        /// 空组(没有 addStep)应当在一帧内结束,不卡住系统。
        /// </summary>
        private void testEmptyGroup()
        {
            Console.WriteLine();
            Console.WriteLine("-- 空组");

            var sys = new TezStepSystem2();
            var group = new TezStepGroup();

            sys.runInNextFrame<TezStepStackExecutor>(group);
            Check(!sys.isEmpty(), "空组入队后系统非空");

            Tick(sys);
            Check(sys.isEmpty(), "空组一帧内完成");
        }

        /// <summary>
        /// setWishedTiming(Wait) 让组停在原地;setWishedTiming(Run) 从下一步继续。
        /// </summary>
        private void testWaitAndResume()
        {
            Console.WriteLine();
            Console.WriteLine("-- Wait 与 Resume");

            var sys = new TezStepSystem2();
            var order = new List<string>();

            var group = new TezStepGroup();
            group.addStep(m =>
            {
                order.Add("a");
                m.setWishedTiming(TezStepWishedTiming.Wait);
            });
            group.addStep(m => order.Add("b"));

            sys.runInNextFrame<TezStepStackExecutor>(group);

            Tick(sys);
            CheckOrder(order, "执行到 a 后进入等待", "a");

            Tick(sys);
            Tick(sys);
            CheckOrder(order, "等待期间不推进", "a");
            Check(!sys.isEmpty(), "等待中的组仍占用系统");

            group.setWishedTiming(TezStepWishedTiming.Run);

            Tick(sys);
            CheckOrder(order, "恢复后从下一步继续", "a", "b");

            Tick(sys);
            Check(sys.isEmpty(), "恢复执行后正常结束");
        }

        /// <summary>
        /// setWishedTiming(Abort) 停止组、触发 evtAbort,后续步骤不执行。
        /// </summary>
        private void testAbort()
        {
            Console.WriteLine();
            Console.WriteLine("-- Abort");

            var sys = new TezStepSystem2();
            var order = new List<string>();
            bool aborted = false;

            var group = new TezStepGroup();
            group.evtAbort += () => aborted = true;
            group.addStep(m =>
            {
                order.Add("a");
                m.setWishedTiming(TezStepWishedTiming.Abort);
            });
            group.addStep(m => order.Add("b"));

            sys.runInNextFrame<TezStepStackExecutor>(group);

            Tick(sys);
            CheckOrder(order, "Abort 之前的那一步正常执行", "a");

            Tick(sys);
            Check(aborted, "Abort 触发 evtAbort");
            CheckOrder(order, "Abort 之后不再执行后续步骤", "a");

            Tick(sys);
            Check(sys.isEmpty(), "Abort 后系统清空");
        }

        /// <summary>
        /// 父组在 wait 处挂起,子组执行完后父组从下一步继续。
        /// </summary>
        private void testWaitChildGroup()
        {
            Console.WriteLine();
            Console.WriteLine("-- 等待子组(Stack)");

            var sys = new TezStepSystem2();
            var order = new List<string>();

            var child = new TezStepGroup();
            child.addStep(m => order.Add("c1"));
            child.addStep(m => order.Add("c2"));

            var parent = new TezStepGroup();
            parent.addStep(m =>
            {
                order.Add("p1");
                m.wait<TezStepStackExecutor>(child);
            });
            parent.addStep(m => order.Add("p2"));

            sys.runInNextFrame<TezStepStackExecutor>(parent);

            Tick(sys);
            CheckOrder(order, "父组第一步执行", "p1");

            Tick(sys);
            CheckOrder(order, "子组开始执行", "p1", "c1");

            Tick(sys);
            CheckOrder(order, "子组继续执行", "p1", "c1", "c2");
            Check(!order.Contains("p2"), "子组未完成时父组保持挂起");

            Tick(sys);
            CheckOrder(order, "子组完成后父组继续", "p1", "c1", "c2", "p2");

            Tick(sys);
            Check(sys.isEmpty(), "整条链结束后系统清空");
        }

        /// <summary>
        /// 回归:同一个组可以连续 wait 两次(子执行器会被复用)。
        /// </summary>
        private void testTwoWaitsOnSameGroup()
        {
            Console.WriteLine();
            Console.WriteLine("-- 同一个组连续两次 wait");

            var sys = new TezStepSystem2();
            var order = new List<string>();

            var c1 = new TezStepGroup();
            c1.addStep(m => order.Add("c1"));

            var c2 = new TezStepGroup();
            c2.addStep(m => order.Add("c2"));

            var parent = new TezStepGroup();
            parent.addStep(m =>
            {
                order.Add("p1");
                m.wait<TezStepStackExecutor>(c1);
            });
            parent.addStep(m =>
            {
                order.Add("p2");
                m.wait<TezStepStackExecutor>(c2);
            });
            parent.addStep(m => order.Add("p3"));

            sys.runInNextFrame<TezStepStackExecutor>(parent);
            RunUntilEmpty(sys);

            Check(sys.isEmpty(), "两次 wait 后系统正常清空");
            CheckOrder(order, "两次 wait 都正确恢复父组",
                "p1", "c1", "p2", "c2", "p3");
        }

        /// <summary>
        /// 三层嵌套:父组等子组,子组等孙组。
        /// </summary>
        private void testNestedWait()
        {
            Console.WriteLine();
            Console.WriteLine("-- 三层嵌套等待");

            var sys = new TezStepSystem2();
            var order = new List<string>();

            var grandchild = new TezStepGroup();
            grandchild.addStep(m => order.Add("g1"));

            var child = new TezStepGroup();
            child.addStep(m => order.Add("c1"));
            child.addStep(m =>
            {
                order.Add("c2");
                m.wait<TezStepStackExecutor>(grandchild);
            });
            child.addStep(m => order.Add("c3"));

            var parent = new TezStepGroup();
            parent.addStep(m =>
            {
                order.Add("p1");
                m.wait<TezStepStackExecutor>(child);
            });
            parent.addStep(m => order.Add("p2"));

            sys.runInNextFrame<TezStepStackExecutor>(parent);
            RunUntilEmpty(sys);

            Check(sys.isEmpty(), "嵌套等待后系统正常清空");
            CheckOrder(order, "三层嵌套按深度正确恢复",
                "p1", "c1", "c2", "g1", "c3", "p2");
        }

        /// <summary>
        /// 序列执行器:队首组跑完自动出队,接着跑下一组。
        /// </summary>
        private void testSequenceExecutorOrder()
        {
            Console.WriteLine();
            Console.WriteLine("-- 序列执行器:顺序执行");

            var sys = new TezStepSystem2();
            var order = new List<string>();

            var g1 = new TezStepGroup();
            g1.addStep(m => order.Add("a1"));
            g1.addStep(m => order.Add("a2"));

            var g2 = new TezStepGroup();
            g2.addStep(m => order.Add("b1"));

            var executor = new TezSequenceStepExecutor();
            var api = (ITezStepExecutor)executor;
            api.add(g1);
            api.add(g2);

            sys.runImmediately(executor);
            RunUntilEmpty(sys);

            Check(sys.isEmpty(), "序列执行器正常清空");
            CheckOrder(order, "先跑完第一组再跑第二组", "a1", "a2", "b1");
        }

        /// <summary>
        /// 回归:序列执行器在"非首组"里 wait 后,要回到同一组继续,而不是从头重跑。
        /// </summary>
        private void testSequenceExecutorWaitInSecondGroup()
        {
            Console.WriteLine();
            Console.WriteLine("-- 序列执行器:第二组中 wait");

            var sys = new TezStepSystem2();
            var order = new List<string>();

            var g1 = new TezStepGroup();
            g1.addStep(m => order.Add("a1"));

            var child = new TezStepGroup();
            child.addStep(m => order.Add("c1"));

            var g2 = new TezStepGroup();
            g2.addStep(m =>
            {
                order.Add("b1");
                m.wait<TezStepStackExecutor>(child);
            });
            g2.addStep(m => order.Add("b2"));

            var executor = new TezSequenceStepExecutor();
            var api = (ITezStepExecutor)executor;
            api.add(g1);
            api.add(g2);

            sys.runImmediately(executor);
            RunUntilEmpty(sys);

            Check(sys.isEmpty(), "序列执行器 wait 后正常清空");
            CheckOrder(order, "恢复时回到第二组继续,不重跑第一组",
                "a1", "b1", "c1", "b2");
        }

        /// <summary>
        /// 序列执行器:某个组 Abort 后出队,继续下一组。
        /// </summary>
        private void testSequenceExecutorAbortAdvances()
        {
            Console.WriteLine();
            Console.WriteLine("-- 序列执行器:Abort 后推进下一组");

            var sys = new TezStepSystem2();
            var order = new List<string>();
            bool aborted = false;

            var g1 = new TezStepGroup();
            g1.evtAbort += () => aborted = true;
            g1.addStep(m =>
            {
                order.Add("a1");
                m.setWishedTiming(TezStepWishedTiming.Abort);
            });
            g1.addStep(m => order.Add("a2"));

            var g2 = new TezStepGroup();
            g2.addStep(m => order.Add("b1"));

            var executor = new TezSequenceStepExecutor();
            var api = (ITezStepExecutor)executor;
            api.add(g1);
            api.add(g2);

            sys.runImmediately(executor);
            RunUntilEmpty(sys);

            Check(sys.isEmpty(), "Abort 后序列执行器正常清空");
            Check(aborted, "被 Abort 的组触发 evtAbort");
            CheckOrder(order, "跳过被 Abort 组的剩余步骤,继续下一组", "a1", "b1");
        }

        /// <summary>
        /// Parent 时序:中断整个执行器,队列里剩余的组不再执行。
        /// </summary>
        private void testParentEndsExecutor()
        {
            Console.WriteLine();
            Console.WriteLine("-- Parent 结束整个执行器");

            var sys = new TezStepSystem2();
            var order = new List<string>();

            var g1 = new TezStepGroup();
            g1.addStep(m =>
            {
                order.Add("a1");
                m.setWishedTiming(TezStepWishedTiming.Parent);
            });
            g1.addStep(m => order.Add("a2"));

            var g2 = new TezStepGroup();
            g2.addStep(m => order.Add("b1"));

            var executor = new TezSequenceStepExecutor();
            var api = (ITezStepExecutor)executor;
            api.add(g1);
            api.add(g2);

            sys.runImmediately(executor);
            RunUntilEmpty(sys);

            Check(sys.isEmpty(), "Parent 后系统清空");
            CheckOrder(order, "Parent 中断执行器,后续组不执行", "a1");
        }

        /// <summary>
        /// runImmediately 会在当帧启动;runInNextFrame 要到下一帧。
        /// </summary>
        private void testRunImmediatelyVsNextFrame()
        {
            Console.WriteLine();
            Console.WriteLine("-- 立即执行与下一帧执行");

            var sys = new TezStepSystem2();
            var order = new List<string>();

            var immediate = new TezStepGroup();
            immediate.addStep(m => order.Add("imm"));

            var next = new TezStepGroup();
            next.addStep(m => order.Add("next"));

            var main = new TezStepGroup();
            main.addStep(m =>
            {
                order.Add("main");
                sys.runImmediately<TezStepStackExecutor>(immediate);
                sys.runInNextFrame<TezStepStackExecutor>(next);
            });
            main.addStep(m => order.Add("main2"));

            sys.runInNextFrame<TezStepStackExecutor>(main);

            Tick(sys);
            CheckOrder(order, "runImmediately 在当帧启动,runInNextFrame 不启动", "main", "imm");

            Tick(sys);
            // 缓存里的执行器在帧首追加到当前列表尾部,所以排在当帧已有执行器之后
            CheckOrder(order, "runInNextFrame 在下一帧启动(排在当帧已有执行器之后)",
                "main", "imm", "main2", "next");

            RunUntilEmpty(sys);
            Check(sys.isEmpty(), "混合调度后系统正常清空");
        }

        #endregion

        #region 断言与驱动

        static int sPassed;
        static int sFailed;
        static readonly List<string> sFailures = new List<string>();

        static void Check(bool condition, string message)
        {
            if (condition)
            {
                sPassed++;
                Console.WriteLine("  ok   " + message);
            }
            else
            {
                sFailed++;
                sFailures.Add(message);
                Console.WriteLine("  FAIL " + message);
            }
        }

        static void CheckOrder(List<string> actual, string message, params string[] expected)
        {
            bool ok = actual.Count == expected.Length;
            if (ok)
            {
                for (int i = 0; i < expected.Length; i++)
                {
                    if (actual[i] != expected[i])
                    {
                        ok = false;
                        break;
                    }
                }
            }

            string detail = ok
                ? string.Empty
                : $" | 实际=[{string.Join(",", actual)}] 期望=[{string.Join(",", expected)}]";

            Check(ok, message + detail);
        }

        /// <summary>
        /// 推进一帧。框架在 update() 里有调试输出,这里临时吞掉。
        /// </summary>
        static void Tick(TezStepSystem2 sys)
        {
            var old = Console.Out;
            Console.SetOut(TextWriter.Null);
            try
            {
                sys.update();
            }
            finally
            {
                Console.SetOut(old);
            }
        }

        /// <summary>
        /// 一直推进到系统为空。超过上限说明有链没有正常结束。
        /// </summary>
        static int RunUntilEmpty(TezStepSystem2 sys, int maxFrames = 64)
        {
            int frames = 0;
            while (!sys.isEmpty() && frames < maxFrames)
            {
                Tick(sys);
                frames++;
            }

            if (!sys.isEmpty())
            {
                Check(false, $"超过 {maxFrames} 帧仍未结束(可能有链卡住)");
            }

            return frames;
        }

        #endregion
    }
}
