using System.Threading;
using Cysharp.Threading.Tasks;

namespace CG
{
    public interface IPlayable
    {
        public UniTask Play(bool fastForward, CancellationToken cancellationToken);

        public UniTask Exit(bool fastForward, CancellationToken cancellationToken);

        public void Skip();

        public void Hide();

        public void Show();
    }
}