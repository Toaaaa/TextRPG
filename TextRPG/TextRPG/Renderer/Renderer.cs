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
    
    // error handler
    private bool _isInvalid = false;
    public void Error() => _isInvalid = true;
    private void _showError() { Logger.WriteLine("\n잘못된 선택입니다.", ConsoleColor.Red); _isInvalid = false; }

    public Renderer(Action<Renderer, States> register)
    {
        register(this, States);
    }

    // do: 잘못된 입력에 안내가 필요한 경우
    public void Render()
    {
        Console.Clear();
        this.Content();
        if (_isInvalid) { _showError(); }
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