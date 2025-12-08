using UnityEngine;

// runs a behaviour tree by evaluating a root node each frame
public class BehaviourTree : MonoBehaviour
{
    private BTNode rootNode;

    // builds the tree when the component awakens
    void Start()
    {
        BuildTree();
    }

    // evaluates the root node every frame
    void Update()
    {
        if (rootNode != null)
        {
            rootNode.Evaluate();
        }
    }

    // override to wire up specific trees in derived classes
    protected virtual void BuildTree()
    {
    }

    // allows other code to assign the root node
    public void SetRootNode(BTNode node)
    {
        rootNode = node;
    }

    // triggers evaluation manually and returns the resulting status
    public BTNodeStatus Execute()
    {
        if (rootNode != null)
        {
            return rootNode.Evaluate();
        }
        return BTNodeStatus.Failure;
    }
}

