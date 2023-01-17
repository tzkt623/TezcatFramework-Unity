using System.Runtime.CompilerServices;
using tezcat.Framework.Exp;

namespace tezcat.Framework.Test
{
    public class TestAwaitable
    {
        private struct Machine : IAsyncStateMachine
        {
            public void MoveNext()
            {
                throw new System.NotImplementedException();
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                throw new System.NotImplementedException();
            }
        }


        public async void test()
        {
            await new TezAwaitable();
        }
    }
}