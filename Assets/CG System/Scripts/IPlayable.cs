using System.Threading;
using Cysharp.Threading.Tasks;

namespace CG
{
    public interface IPlayable
    {
        public UniTask Enter(CancellationToken cancellationToken);

        public UniTask Exit(CancellationToken cancellationToken);

        public void Skip() { }

        public void Hide() { }

        public void Show() { }
    }
}