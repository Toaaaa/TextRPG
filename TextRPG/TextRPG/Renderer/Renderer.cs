public class Renderer
{
    public States States { get; private set; } = new States();

    // 라이프사이클
    // learn: 초기 값이 없는 경우, 렌더링 시 에러 발생 가능
    public Action? Mount = null;
    public Action Content = null!;
    public Action Choice = null!;
    public dynamic? Selection { get; set; }
    
    // result
    private bool _isInvalid = false;
    public void Error() => _isInvalid = true;

    public Renderer(Action<Renderer, States> register)
    {
        register(this, States);
    }

    // public void Choice<T>(Action<int> choice, State<T> state, T value)
    // {
    //     // Chocies.Add(());
    // }

    // do: 잘못된 입력에 안내가 필요한 경우
    public void Render()
    {
        Console.Clear();
        this.Content();
        if (_isInvalid)
        {
            Logger.WriteLine("\n잘못된 선택입니다.", ConsoleColor.Red);
            _isInvalid = false;
        }
        // fix: 선택 문구도 달라질 수 있음.
        // Console.WriteLine("\n원하시는 행동을 입력해주세요.\n>>");
        string? input = Console.ReadLine();
        if (int.TryParse(input, out int selection))
        {
            this.Selection = selection;
        }
        else if(!string.IsNullOrWhiteSpace(input))
        {
            this.Selection = input!;
        }
        this.Choice();
        this.Render();
    }
}