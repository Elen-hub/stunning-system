public class CircularLinkedListNode<T>
{
    T _value;
    public T GetValue => _value;
    public CircularLinkedListNode<T> Prev { get; set; }
    public CircularLinkedListNode<T> Next { get; set; }
    public CircularLinkedListNode(T value)
    {
        _value = value;
    }
}
