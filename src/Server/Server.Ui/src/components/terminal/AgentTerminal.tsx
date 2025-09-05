import React, { useEffect, useRef } from "react";
import { Terminal } from "xterm";
import { FitAddon } from "xterm-addon-fit";
import "xterm/css/xterm.css";

interface AgentTerminalProps {
  agentId: string;
}

export const AgentTerminal: React.FC<AgentTerminalProps> = ({ agentId }) => {
  const terminalRef = useRef<HTMLDivElement>(null);
  const termRef = useRef<Terminal | null>(null); // ✅
  const fitAddonRef = useRef<FitAddon | null>(null);

  useEffect(() => {
    const term = new Terminal({
      cursorBlink: true,
      fontFamily: "monospace",
      fontSize: 14,
      theme: { background: "#000", foreground: "#0f0" },
    });
    const fitAddon = new FitAddon();

    term.loadAddon(fitAddon);

    if (terminalRef.current) {
      term.open(terminalRef.current);
      fitAddon.fit();
      term.writeln(`Connected to agent ${agentId}`);
      term.writeln(`Type "help" for commands\n`);
    }

    term.onKey(e => {
      const char = e.key;
      if (char === "\r") {
        term.writeln("");
        term.writeln("Command executed (demo)");
        term.write("$ ");
      } else if (char === "\u007F") {
        term.write("\b \b");
      } else {
        term.write(char);
      }
    });

    termRef.current = term;
    fitAddonRef.current = fitAddon;

    return () => {
      term.dispose();
    };
  }, [agentId]);

  return <div ref={terminalRef} style={{ width: "100%", height: "400px" }} />;
};
