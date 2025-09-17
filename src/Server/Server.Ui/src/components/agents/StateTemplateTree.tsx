import 'reactflow/dist/style.css';
import { useEffect, useState } from "react";
import { AgentStateNodeDto, AgentStateDto } from "../../types/state";
import ReactFlow, { Node, Edge, MarkerType, PanOnScrollMode } from 'reactflow';

interface Props {
    template: AgentStateNodeDto;
    activeState?: AgentStateDto | null;
}


const flattenTree = (
    node: AgentStateNodeDto,
    parentPath: string = ""
): { nodes: Node[]; edges: Edge[] } => {
    let nodes: Node[] = [];
    let edges: Edge[] = [];

    const nodeId = `${parentPath}${node.machineType}_${node.name}`;

    nodes.push({
        id: nodeId,
        data: { label: node.name, machineType: node.machineType },
        position: { x: Number(node.x) || 0, y: Number(node.y) || 0 },
        style: {
            background: '#fff',
            color: '#000',
            border: '1px solid #222',
            borderRadius: 5,
            padding: 10
        }
    });

    node.transitions.forEach(t => {
        edges.push({
            id: `${nodeId}->${parentPath}${node.machineType}_${t}`,
            source: nodeId,
            target: `${parentPath}${node.machineType}_${t}`,
            type: 'step',
            style: { strokeWidth: 2, stroke: '#555' },
            markerEnd: { type: MarkerType.ArrowClosed },
            animated: false
        });
    });

    node.machines.forEach(child => {
        const { nodes: childNodes, edges: childEdges } = flattenTree(
            child,
            nodeId + "/"
        );
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
            prev.map(node => {
                const isActive =
                    node.data.machineType?.includes(activeState.machine.replace("Agent", "")) &&
                    node.id.endsWith(`_${activeState.toState}`);
                return {
                    ...node,
                    style: {
                        ...node.style,
                        background: isActive ? '#4caf50' : '#fff',
                        color: isActive ? '#fff' : '#000'
                    }
                };
            })
        );

        setEdges(prev =>
            prev.map(edge => {
                const isActive =
                    edge.source.endsWith(`_${activeState.fromState}`) &&
                    edge.target.endsWith(`_${activeState.toState}`);
                return {
                    ...edge,
                    animated: isActive,
                    style: {
                        ...edge.style,
                        stroke: isActive ? '#4caf50' : '#555'
                    }
                };
            })
        );
    }, [activeState]);



    return (
        <div style={{ width: "100%", height: "500px" }}>
            <ReactFlow
                nodes={nodes}
                edges={edges}
                nodesDraggable={false}
                nodesConnectable={false}
                panOnScrollMode={PanOnScrollMode.Free}
            />
        </div>
    );
};
