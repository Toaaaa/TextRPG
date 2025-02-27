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
        if (!_page.Scenes.ContainsKey(pageId)) return;
        Renderer currentScene = _page.Scenes[pageId];
        currentScene.LazyLoad();
        currentScene.States.Clear();
        currentScene.Mount?.Invoke();
        currentScene.Render();
    }
    
    public void Navigate(dynamic pageId, object locationState)
    {
        // err: 페이지가 없는 경우
        // fix: SetRouter 호출과 동작엔 문제 없지만 보장하기 어려움.
        _history.Push(pageId);
        if (!_page.Scenes.ContainsKey(pageId)) return;
        Renderer currentScene = _page.Scenes[pageId];
        currentScene.LazyLoad();
        currentScene.States.Clear();
        currentScene.States.SetParams(locationState);
        currentScene.Mount?.Invoke();
        currentScene.Render();
    }

    // 만일 history가 동적인 경우 ReplaceState를 구현해서 해소.
    public void PopState(int count =  1)
    {
        for (int i = 0; i < count; i++)
        {
            _history.Pop();
        }
        dynamic previousId = _history.Peek();
        Renderer currentScene = _page.Scenes[previousId];
        // do: 넘어갈 때 상태를 지우는 게 맞으나 스택이 비어있을 수 있다.
        currentScene.States.Clear();
        currentScene.Render();
    }
}