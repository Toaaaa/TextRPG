public class Renderer
{
    public States States { get; private set; } = new States();

    // life cycle
    // learn: 초기 값이 없는 경우, 렌더링 시 에러 발생 가능
    public Action? Mount = null;
    public Action Content = null!;
    public Action Choice = null!;
    
    // selection
    public int Selection { get; set; }
    public string SelectionText { get; set; } = string.Empty;
    public enum SelectionType { number, text }
    public SelectionType SelectionMode = SelectionType.number;
    public Action<Renderer, States>? Register;
    
    // error handler
    private string? _invalidateMessage = null;
    public void Error(string? message = "잘못된 선택입니다.") => _invalidateMessage = message;
    private void _showError() { Logger.WriteLine(_invalidateMessage, ConsoleColor.Red); _invalidateMessage = null; }

    public Renderer(Action<Renderer, States> register)
    {
        // register(this, States);
        Register = register;
    }

    public void LazyLoad()
    {
        if (Register != null)
        {
            Register(this, States);
            Register = null;
        }
    }

    // do: 잘못된 입력에 안내가 필요한 경우
    public void Render()
    {
        Console.Clear();
        this.Content();
        if (_invalidateMessage != null) { _showError(); }
        // fix: 선택 문구도 달라질 수 있음.
        // Console.WriteLine("\n원하시는 행동을 입력해주세요.\n>>");
        string? input = Console.ReadLine();

        switch (SelectionMode)
        {
            case SelectionType.number:
                if (int.TryParse(input, out int selection))
                {
                    this.Selection = selection;
                    this.Choice();
                    break;
                }
                Error();
                break;
            case SelectionType.text:
                if(!string.IsNullOrWhiteSpace(input))
                {
                    this.SelectionText = input;
                    this.Choice();
                    break;
                }
                Error();
                break;
        }
        
        this.Render();
    }
}