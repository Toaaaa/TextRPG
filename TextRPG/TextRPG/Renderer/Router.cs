public class Router
{
    // q: 페이지 타입을 page로부터 받을 수 없는 지
    private readonly Stack<dynamic> _history = new Stack<dynamic>();
    private readonly Page _page;

    public Router(Page page)
    {
        this._page = page;
        _page.SetRouter(this);
    }
    
    public void Navigate(dynamic pageId)
    {
        _history.Push(pageId);
        // err: 페이지가 없는 경우
        if (!_page.Scenes.ContainsKey(pageId)) return;
        // fix: SetRouter 호출과 동작엔 문제 없지만 보장하기 어려움.
        Renderer currentScene = _page.Scenes[pageId];
        currentScene.Clear();
        currentScene.Render();
    }
    
    public void Navigate<T>(dynamic pageId, T locationState)
    {
        _history.Push(pageId);
        // err: 페이지가 없는 경우
        if (!_page.Scenes.ContainsKey(pageId)) return;
        // fix: SetRouter 호출과 동작엔 문제 없지만 보장하기 어려움.
        Renderer currentScene = _page.Scenes[pageId];
        currentScene.SetLocation<T>(locationState);
        currentScene.Clear();
        currentScene.Render();
    }

    // 만일 history가 동적인 경우 ReplaceState를 구현해서 해소.
    public void PopState()
    { 
        dynamic currentId = _history.Peek();
        // err: 없는 에러가 발생하는 경우
        Renderer currentScene = _page.Scenes[currentId];
        _history.Pop();
        
        dynamic previousId = _history.Peek();
        currentScene = _page.Scenes[previousId];
        // do: 넘어갈 때 상태를 지우는 게 맞으나 스택이 비어있을 수 있다.
        currentScene.Clear();
        currentScene.Render();
    }
}