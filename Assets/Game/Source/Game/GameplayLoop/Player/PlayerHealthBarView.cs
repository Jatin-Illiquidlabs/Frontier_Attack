using UniRx;
using UnityEngine;
using Zenject;

namespace WerewolfBearer {
    public class PlayerHealthBarView : MonoBehaviour {
        [SerializeField]
        private RectTransform _healthBarValueRectTransform;

        [Inject]
        private PlayerCharacterModel _playerCharacterModel;

        private void OnEnable() {
            _playerCharacterModel.Health
            .Merge(_playerCharacterModel.MaxHealth)
            .TakeUntilDisable(this)
            .Subscribe(_ => {
                _healthBarValueRectTransform.anchorMax =
                    new Vector2(_playerCharacterModel.Health.Value / _playerCharacterModel.MaxHealth.Value, 1);
            });
        }
    }
}