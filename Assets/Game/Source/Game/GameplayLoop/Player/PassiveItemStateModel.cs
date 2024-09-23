namespace WerewolfBearer {
    public class PassiveItemStateModel {
        public PassiveItemDefinition Definition { get; }
        public int Level { get; set; }

        public PassiveItemStateModel(PassiveItemDefinition definition) {
            Definition = definition;

            Level = 1;
        }
    }
}
