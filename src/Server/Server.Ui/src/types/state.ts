

export interface AgentStateDto {
    machine: string;
    fromState: string;
    toState: string;
    trigger: string;
    timestamp: string;
    details: string;
}


export interface AgentStateNodeDto {
    name: string;
    transitions: string[];
    machines: AgentStateNodeDto[];
    machineType: string;
    x: string;
    y: string;
}