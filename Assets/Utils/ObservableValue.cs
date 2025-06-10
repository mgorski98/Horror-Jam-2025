using UnityEngine;
using UnityEngine.Events;

namespace Assets.Utils {
    [System.Serializable]
    public class ObservableValue<T> {
        [SerializeField]
        private T _value;
        public T Value {
            get => _value;
            set {
                var old = _value;
                _value = value;
                if (!(old?.Equals(value) ?? false)) {
                    OnValueChanged.Invoke(old, _value);
                }
            }
        }

        public UnityEvent<T, T> OnValueChanged = new();

        public ObservableValue() {
            this.Value = default(T);
        }

        public ObservableValue(T value) {
            SetValueWithoutNotify(value);
        }

        public void SetValueWithoutNotify(T value) {
            this._value = value;
        }

        public static implicit operator T(ObservableValue<T> ov) {
            return ov == null ? default(T) : ov.Value;
        }

        public override string ToString() {
            return _value.ToString();
        }
    }
}
