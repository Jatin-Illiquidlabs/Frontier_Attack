namespace WerewolfBearer {
    public interface IEnemyEventsListener {
        void OnTouchedPlayer(EnemyController enemyController);
        void OnTouchedPlayerWeapon(EnemyController enemyController, AttackData attackData);
    }
}
