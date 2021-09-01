using System.Threading.Tasks;
using Grpc.Core;

namespace ArkLike.HookServer.Core.Producer
{
	public class ProducerService : Producer.ProducerBase
	{
		public override Task SubscribeBattleReplay(IAsyncStreamReader<BattelReplayRequest> requestStream, IServerStreamWriter<BattleReplayResponse> responseStream,
			ServerCallContext context)
		{

			return Task.CompletedTask;
		}
	}
}