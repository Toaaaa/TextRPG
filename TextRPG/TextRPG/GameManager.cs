namespace TextRPG;

public class GameManager
{
    Router Router = new Router(new Page());
    
    public void StartGame()
    {
        this.Router.Navigate(PageType.INIT_PAGE);
    }
}