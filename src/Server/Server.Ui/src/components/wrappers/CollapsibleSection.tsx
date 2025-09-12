import { useState, ReactNode } from "react";
import { Box, Card, CardContent, Collapse, IconButton, Typography, Badge } from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";

interface CollapsibleSectionProps {
    title: string;
    defaultOpen?: boolean;
    children: ReactNode;
    noCard?: boolean;
    notification?: boolean | ReactNode;
}

export const CollapsibleSection = ({
    title,
    defaultOpen = true,
    children,
    noCard,
    notification
}: CollapsibleSectionProps) => {
    const [open, setOpen] = useState(defaultOpen);

    return (
        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
            <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between" }}>
                <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                    <Typography variant="h6">{title}</Typography>
                    {notification && (
                        <Badge
                            color="error"
                            variant="dot"
                            sx={{ "& .MuiBadge-dot": { height: 10, minWidth: 10, borderRadius: "50%" } }}
                        >
                            {typeof notification !== "boolean" ? notification : null}
                        </Badge>
                    )}
                </Box>

                <IconButton size="small" onClick={() => setOpen(!open)}>
                    <ExpandMoreIcon
                        sx={{
                            transform: open ? "rotate(180deg)" : "rotate(0deg)",
                            transition: "transform 0.3s"
                        }}
                    />
                </IconButton>
            </Box>

            <Collapse in={open} timeout="auto" unmountOnExit>
                {!noCard ? (
                    <Card>
                        <CardContent>{children}</CardContent>
                    </Card>
                ) : (
                    children
                )}
            </Collapse>
        </Box>
    );
};
