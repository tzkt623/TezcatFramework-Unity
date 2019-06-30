using UnityEngine;
using System.Collections;
using tezcat.Framework.Math;

namespace tezcat.Framework.AI
{
    public class TezBTTestData : ITezBTData
    {
        public void close()
        {

        }
    }

    public class TezTestBT : TezBehaviorTree<TezBTTestData>
    {

    }

    public class TezTestSelectorNode : TezBTSelectorNode<TezBTTestData>
    {
        public override TezBTResult execute(TezBTTestData data)
        {
            Debug.Log(string.Format("BT : {0}[{1}]", this.name, this.GetType().Name));
            return base.execute(data);
        }
    }

    public class TezTestParallelNode : TezBTParallelNode<TezBTTestData>
    {
        public override TezBTResult execute(TezBTTestData data)
        {
            Debug.Log(string.Format("BT : {0}[{1}]", this.name, this.GetType().Name));
            return base.execute(data);
        }
    }

    public class TezTestSequenceNode : TezBTSequenceNode<TezBTTestData>
    {
        public override TezBTResult execute(TezBTTestData data)
        {
            Debug.Log(string.Format("BT : {0}[{1}]", this.name, this.GetType().Name));
            return base.execute(data);
        }
    }

    public class TezTestActionNode : TezBTActionNode<TezBTTestData>
    {
        TezRandom random = new TezRandom();

        public override void close()
        {

        }

        public override TezBTResult execute(TezBTTestData data)
        {
            Debug.Log(string.Format("BT : {0}[{1}]", this.name, this.GetType().Name));

            var rate = random.nextInt(0, 100);

            if (rate > 80)
            {
                return TezBTResult.Success;
            }

            if (rate < 10)
            {
                return TezBTResult.Fail;
            }

            return TezBTResult.Running;
        }
    }

    public class TezBTTest
    {
        static TezBTTestData data = new TezBTTestData();
        static TezTestBT bt = new TezTestBT();

        public static void init()
        {
            var selector = new TezTestSelectorNode()
            {
                name = "Selector 1"
            };
            bt.setNode(selector);


            var parallel_and = new TezTestParallelNode()
            {
                algorithm = TezTestParallelNode.Algorithm.And,
                name = "Parallel 1 [And]"
            };
            parallel_and.addNode(new TezTestActionNode()
            {
                name = "Action 1"
            });
            parallel_and.addNode(new TezTestActionNode()
            {
                name = "Action 2"
            });

            var parallel_or = new TezTestParallelNode()
            {
                algorithm = TezTestParallelNode.Algorithm.Or,
                name = "Parallel 2 [Or]"
            };
            parallel_or.addNode(new TezTestActionNode()
            {
                name = "Action 3"
            });
            parallel_or.addNode(new TezTestActionNode()
            {
                name = "Action 4"
            });
            parallel_or.addNode(new TezTestActionNode()
            {
                name = "Action 5"
            });

            parallel_and.addNode(parallel_or);


            var sequence = new TezTestSequenceNode()
            {
                name = "Sequence 1"
            };

            sequence.addNode(new TezTestActionNode()
            {
                name = "Action 6"
            });
            sequence.addNode(new TezTestActionNode()
            {
                name = "Action 7"
            });
            sequence.addNode(new TezTestActionNode()
            {
                name = "Action 8"
            });

            selector.addNode(parallel_and);
            selector.addNode(sequence);
            selector.addNode(new TezTestActionNode()
            {
                name = "Action 9"
            });

        }

        public static void execute()
        {
            switch (bt.execute(data))
            {
                case TezBTResult.Fail:
                    Debug.Log("Fail");
                    break;
                case TezBTResult.Running:
                    Debug.Log("Running");
                    break;
                case TezBTResult.Success:
                    Debug.Log("Success");
                    break;
            }
        }
    }
}