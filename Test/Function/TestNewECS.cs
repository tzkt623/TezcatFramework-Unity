using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Test
{
    struct float3
    {
        public static float3 zero => new float3() { x = 0, y = 0, z = 0 };
        public static float3 one => new float3() { x = 1, y = 1, z = 1 };

        public float x, y, z;
    }

    class ComTransform : TezECS.Component
    {
        public float3 position;
        public float3 rotation;
        public float3 scale;

        public override void onOtherComponentAdded(TezECS.Component other)
        {

        }

        public override void onOtherComponentRemoved(TezECS.Component other)
        {

        }

        public void print()
        {
            Console.WriteLine($"Transform[{this.entityID}]");
        }
    }

    class ComRenderer : TezECS.Component
    {
        public string mesh;
        public string material;

        public override void onOtherComponentAdded(TezECS.Component other)
        {

        }

        public override void onOtherComponentRemoved(TezECS.Component other)
        {

        }

        public void print()
        {
            Console.WriteLine($"Render[{this.entityID}] {this.mesh} with {this.material}");
        }
    }

    class ComCamera : TezECS.Component
    {
        public float fov;
        public float near;
        public float far;

        public override void onOtherComponentAdded(TezECS.Component other)
        {

        }

        public override void onOtherComponentRemoved(TezECS.Component other)
        {

        }

        public void print()
        {
            Console.WriteLine($"Camera[{this.entityID}] fov:{this.fov} near:{this.near} far:{this.far}");
        }
    }

    class SystemRender : TezECS.System
    {
        public SystemRender()
        {
            this.addUpdater(this.update);
        }

        public void update()
        {
            var manaegr = TezECS.getComponentManager<ComRenderer>();
            foreach (var renderer in manaegr.components)
            {
                renderer.print();
            }
        }
    }

    class SystemCamera : TezECS.System
    {
        public SystemCamera()
        {
            this.addUpdater(this.update);
        }

        public void update()
        {
            var manager = TezECS.getComponentManager<ComCamera>();
            foreach (var camera in manager.components)
            {
                camera.print();
            }
        }
    }

    class SystemTransform : TezECS.System
    {
        public SystemTransform()
        {
            this.addUpdater(this.update);
        }

        public void update()
        {
            var manager = TezECS.getComponentManager<ComTransform>();
            foreach (var transform in manager.components)
            {
                transform.print();
            }
        }
    }

    public class TestNewECS : TezBaseTest
    {
        SystemRender mSystemRender = null;
        SystemCamera mSystemCamera = null;
        SystemTransform mSystemTransform = null;

        public TestNewECS() : base("ECS")
        {

        }

        private TezECS.Entity createCamera()
        {
            var entiy_camera = TezECS.createEntity();

            var com_camera = TezECS.addComponent<ComCamera>(entiy_camera);
            com_camera.fov = 60;
            com_camera.near = 0.1f;
            com_camera.far = 1000;

            var com_transform = TezECS.addComponent<ComTransform>(entiy_camera);
            com_transform.position = new float3() { x = 0, y = 0, z = -10 };
            com_transform.rotation = float3.zero;
            com_transform.scale = float3.one;

            return entiy_camera;

        }

        private TezECS.Entity createCube()
        {
            var entity = TezECS.createEntity();

            var com_renderer = TezECS.addComponent<ComRenderer>(entity);
            com_renderer.mesh = $"Cube[{entity.id}]";
            com_renderer.material = "Default";

            var com_transform = TezECS.addComponent<ComTransform>(entity);
            com_transform.position = float3.zero;
            com_transform.rotation = float3.zero;
            com_transform.scale = float3.one;

            return entity;
        }

        public override void init()
        {
            mSystemRender = new SystemRender();
            mSystemCamera = new SystemCamera();
            mSystemTransform = new SystemTransform();
        }

        public override void run()
        {
            Random random = new Random();

            int flag = 10;
            List<TezECS.Entity> entities = new List<TezECS.Entity>();
            Console.WriteLine("Press ESC to stop, Press OtherKey to test");
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                var base_count = random.Next(0, TezECS.entityCount > flag ? TezECS.entityCount : flag);
                while (TezECS.entityCount < base_count)
                {
                    switch (random.Next(0, 2))
                    {
                        case 0:
                            entities.Add(this.createCube());
                            break;
                        case 1:
                            entities.Add(this.createCamera());
                            break;
                    }
                }

                for (int i = 0; i < TezECS.freeCount; i++)
                {
                    switch (random.Next(0, 3))
                    {
                        case 0:
                            entities.Add(this.createCube());
                            break;
                        case 1:
                            entities.Add(this.createCamera());
                            break;
                    }
                }

                var begin = random.Next(0, entities.Count);
                var end = random.Next(begin, entities.Count) - 1;
                for (int i = end; i >= begin; i--)
                {             
                    TezECS.recycleEntity(entities[i].id);
                    Console.WriteLine($"Recycle[{entities[i].id}]");
                    entities.RemoveAt(i);
                }

                TezECS.updateSystem();
                Console.WriteLine($"End EntitySlot[{TezECS.entityCount}]================================");
            }
        }

        protected override void onClose()
        {
            TezECS.destoryAllData();
            mSystemRender = null;
        }
    }
}