using Cysharp.Threading.Tasks;
using System.Threading;

namespace fantec.Battle.Root
{
    public interface IRootInitiater
    {
        UniTask InitializeAsync(CancellationTokenSource cts);
    }
}