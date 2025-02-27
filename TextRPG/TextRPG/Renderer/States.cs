public class States
{
    // 상태
    private List<object> _states = new List<object>();
    private object? _locationState;
    
    public State<T> Get<T>(string selectedKey)
    {
        State<T>? state = _states.OfType<State<T>>().FirstOrDefault(state => state.GetKey() == selectedKey);
        if (state == null)
        {
            State<T> newState = new State<T>();
            newState.SetKey(selectedKey);
            _states.Add(newState);
            return newState;
        };
        return state;
    }
    
    // 상태 과련
    public T GetParams<T>()
    {
        return ((T)this._locationState!)!;
    }

    // 굳이 있는 이유: 타입으로 전달
    public void SetParams(object locationState)
    {
        _locationState = locationState;
    }

    public void Dispatch()
    {
        foreach (var state in _states)
        {
            ((dynamic)state).Allocate();
        }
    }

    // 클리어의 개념이 달라짐
    public void Clear()
    {
        foreach (var state in _states)
        {
            // 바로 접근 어려움.
            ((dynamic)state).Reset();
        }
        _locationState = null;
    }
}