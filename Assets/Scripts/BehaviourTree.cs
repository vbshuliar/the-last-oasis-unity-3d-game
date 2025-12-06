using UnityEngine;

public class BehaviourTree : MonoBehaviour
{
    private BTNode rootNode;

    void Start()
    {
        // Build the behaviour tree
        BuildTree();
    }

    void Update()
    {
        if (rootNode != null)
        {
            rootNode.Evaluate();
        }
    }

    protected virtual void BuildTree()
    {
        // This should be overridden in derived classes to build specific trees
    }

    public void SetRootNode(BTNode node)
    {
        rootNode = node;
    }

    public BTNodeStatus Execute()
    {
        if (rootNode != null)
        {
            return rootNode.Evaluate();
        }
        return BTNodeStatus.Failure;
    }
}

