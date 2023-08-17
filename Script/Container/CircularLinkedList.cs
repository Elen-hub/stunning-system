public class CircularLinkedList<T>
{
    public CircularLinkedListNode<T> Head { get; private set; }
    public CircularLinkedListNode<T> End { get; private set; }
    public CircularLinkedListNode<T> AddFirst(T value)
    {
        CircularLinkedListNode<T> node = new CircularLinkedListNode<T>(value);
        if(Head != null)
        {
            Head.Prev = node;
            node.Prev = End;
            node.Next = Head;
            Head = node;
            End.Next = Head;
        }
        else
        {
            Head = node;
            End = node;
            node.Next = node;
            node.Prev = node;
        }
        return node;
    }
    public CircularLinkedListNode<T> AddLast(T value)
    {
        CircularLinkedListNode<T> node = new CircularLinkedListNode<T>(value);
        if (End != null)
        {
            End.Next = node;
            node.Next = Head;
            node.Prev = End;
            End = node;
            Head.Prev = End;
        }
        else
        {
            Head = node;
            End = node;
            node.Next = node;
            node.Prev = node;
        }
        return node;
    }
    public CircularLinkedListNode<T> AddNext(CircularLinkedListNode<T> selectNode, T value)
    {
        CircularLinkedListNode<T> next = selectNode.Next;
        CircularLinkedListNode<T> node = new CircularLinkedListNode<T>(value);
        node.Prev = selectNode;
        node.Next = next;

        selectNode.Next = node;
        next.Prev = node;

        if(End == selectNode)
            End = node;

        return node;
    }
}
