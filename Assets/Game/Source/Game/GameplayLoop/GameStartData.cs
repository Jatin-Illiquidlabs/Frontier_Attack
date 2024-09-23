using UnityEngine.Scripting;

namespace WerewolfBearer {
    [Preserve]
    public class GameStartData {
        public StageId StageId { get; set; }
        public CharacterId CharacterId { get; set; }
    }
}
