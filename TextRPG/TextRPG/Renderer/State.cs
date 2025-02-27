public class State<T>()
{
    private string _key;
    private T _defaultValue;
    private T _value;
    private T _memoValue;
    // private bool isInitialized = false;

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
        this._defaultValue = newValue;
        this._memoValue = newValue;
        this._value = newValue;
        return this;
    }

    public void Reset()
    {
        this._value = _defaultValue;
    }

    public T GetValue()
    {
        return this._value;
    }
    
    public void Allocate()
    {
        this._value = this._memoValue;
    }
    
    public void SetValue(T newValue)
    {
        this._memoValue = newValue;
    }
    
    public void SetValue(Func<T, T> newFunc)
    {
        this._memoValue = newFunc(this._value);
    }
}