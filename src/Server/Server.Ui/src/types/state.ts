

export interface AgentStateDto {
    machine: string;
    fromState: string;
    toState: string;
    trigger: string;
    timestamp: string;
}


export interface AgentStateNodeDto {
    name: string;
    transitions: string[];
    machines: AgentStateNodeDto[];
    x: string;
    y: string;
}