public class Renderer()
{
    private List<object> _states = new List<object>();
    private dynamic LocationState = "";

    public T GetLocationState<T>()
    {
        try
        {
            return (T)Convert.ChangeType(LocationState, typeof(T));
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Cannot convert LocationState to type {typeof(T)}.");
        }
    }

    public void SetLocation<T>(T locationState)
    {
        LocationState = locationState;
    }

    public State<T> State<T>(string selectedKey)
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

    public void Clear()
    {
        _states.Clear();
        // fix: null 인 경우 텍스트 호출 시 문제 발생할 수 있음
        LocationState = "";
    }
    
    // learn: 초기 값이 없는 경우, 렌더링 시 에러 발생 가능
    private Action<Renderer> _contents = null!;
    private Action<Renderer, int> _choices = null!;

    // do: 잘못된 입력에 안내가 필요한 경우
    public void Render()
    {
        Console.Clear();
        this._contents(this);
        // fix: 선택 문구도 달라질 수 있음.
        // Console.WriteLine("\n원하시는 행동을 입력해주세요.\n>>");
        string? input = Console.ReadLine();
        if (int.TryParse(input, out int selection))
        {
            this._choices(this, selection);
        }
        this.Render();
    }

    public Renderer Contents(Action<Renderer> contents)
    {
        this._contents = contents;
        return this;
    }

    public Renderer Choices(Action<Renderer, int> choices)
    {
        this._choices = choices;
        return this;
    }
}