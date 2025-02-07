public class State<T>()
{
    private string _key;
    private T _value;
    private bool isInit = false;

    public string GetKey()
    {
        return this._key;
    }
    
    public void SetKey(string newKey)
    {
        this._key = newKey;
    }
    
    public State<T> Init(T newValue)
    {
        if(isInit) return this;
        this._value = newValue;
        isInit = true;
        return this;
    }

    public T GetValue()
    {
        return this._value;
    }
    
    public void SetValue(T newValue)
    {
        this._value = newValue;
    }
    
    public void SetValue(Func<T, T> newFunc)
    {
        this._value = newFunc(this._value);
    }
}