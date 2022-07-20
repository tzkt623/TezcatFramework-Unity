﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Test
{
    class MyData : TezDataComponent
    {

    }

    class MyRenderer : TezRendererComponent
    {
        public override void reset() { }
        protected override void initObject() { }
        protected override void onHide() { }
        protected override void onShow() { }
        protected override void preInit() { }
    }

    class MyPhysics : ITezComponent
    {
        public static int SComUID;
        public int comUID => SComUID;

        public TezEntity entity { get; private set; }

        public void close()
        {
            this.entity = null;
        }

        public void onAdd(TezEntity entity)
        {
            this.entity = entity;
        }

        public void onOtherComponentAdded(ITezComponent component, int comID)
        {

        }

        public void onOtherComponentRemoved(ITezComponent component, int comID)
        {

        }

        public void onRemove(TezEntity entity)
        {
        }
    }

    class Main
    {
        /// <summary>
        /// must first invoke
        /// </summary>
        public void registerComponent()
        {
            TezDataComponent.SComUID = TezComponentManager.register<TezDataComponent>();
            TezRendererComponent.SComUID = TezComponentManager.register<TezRendererComponent>();
            MyPhysics.SComUID = TezComponentManager.register<MyPhysics>();
        }

        /// <summary>
        /// and then
        /// </summary>
        public void create()
        {
            var entity = TezEntity.create();
            entity.addComponent(new MyData());
            entity.addComponent(new MyRenderer());
            entity.addComponent(new MyPhysics());
        }
    }
}