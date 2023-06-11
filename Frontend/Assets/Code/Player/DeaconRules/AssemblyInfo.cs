using System.Runtime.CompilerServices;
using Unity.Jobs;
using Code.Player.DeaconRules;
using Code.Player.MCTS;

[assembly: InternalsVisibleTo("DeaconRules-tests")]
[assembly: RegisterGenericJobType(typeof(RolloutJob<Player, GameState, GameAction>))]
