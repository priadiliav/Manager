import 'reactflow/dist/style.css';
import { useEffect, useState } from "react";
import { AgentStateNodeDto, AgentStateDto } from "../../types/state";
import ReactFlow, { Node, Edge, MarkerType, PanOnScrollMode } from 'reactflow';

interface Props {
    template: AgentStateNodeDto;
    activeState?: AgentStateDto | null;
}

const flattenTree = (node: AgentStateNodeDto): { nodes: Node[], edges: Edge[] } => {
    let nodes: Node[] = [];
    let edges: Edge[] = [];

    const nodeId = node.name;
    nodes.push({
        id: nodeId,
        data: { label: node.name },
        position: { x: Number(node.x) || 0, y: Number(node.y) || 0 },
        style: {
            background: '#fff',
            color: '#000',
            border: '1px solid #222',
            borderRadius: 5,
            padding: 10
        }
    });

    node.transitions.forEach((t, i) => {
        edges.push({
            id: `${nodeId}-${t}`,
            source: nodeId,
            target: t,
            type: 'step',
            style: { strokeWidth: 2, stroke: '#555' },
            markerEnd: { type: MarkerType.ArrowClosed },
            animated: false
        });
    });

    node.machines.forEach(child => {
        const { nodes: childNodes, edges: childEdges } = flattenTree(child);
        nodes = nodes.concat(childNodes);
        edges = edges.concat(childEdges);
    });

    return { nodes, edges };
};

export const StateTemplateTree = ({ template, activeState }: Props) => {
    const { nodes: initialNodes, edges: initialEdges } = flattenTree(template);

    const [nodes, setNodes] = useState<Node[]>(initialNodes);
    const [edges, setEdges] = useState<Edge[]>(initialEdges);

    useEffect(() => {
        if (!activeState) return;

        setNodes(prev =>
            prev.map(node => ({
                ...node,
                style: {
                    ...node.style,
                    background: node.id === activeState.toState ? '#4caf50' : '#fff',
                    color: node.id === activeState.toState ? '#fff' : '#000'
                }
            }))
        );

        setEdges(prev =>
            prev.map(edge => ({
                ...edge,
                animated:
                    edge.source === activeState.fromState &&
                    edge.target === activeState.toState
            }))
        );
    }, [activeState]);

    return (
        <div style={{ width: "100%", height: "500px" }}>
            <ReactFlow
                nodes={nodes}
                edges={edges}
                nodesDraggable={false}
                nodesConnectable={false}
                panOnDrag={false}
                panOnScroll={false}
                zoomOnScroll={false}
                zoomOnPinch={false}
                panOnScrollMode={PanOnScrollMode.Free}
            />
        </div>
    );
};
