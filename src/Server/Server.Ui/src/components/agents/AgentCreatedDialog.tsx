import { IconButton, TextField, Typography } from "@mui/material";
import { AgentCreateResponse } from "../../types/agent";
import CustomDialog from "../dialogs/CustomDialog";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";

export interface Props {
    secretWindowOpen: boolean;
    handleClose: () => void;
    createdAgentResponse: AgentCreateResponse | null;
}

export const AgentCreatedDialog = (props: Props) => {
    const { secretWindowOpen, createdAgentResponse, handleClose } = props;

    const handleCopySecret = () => {
        if (createdAgentResponse?.secret) {
            navigator.clipboard.writeText(createdAgentResponse.secret);
        }
    };

    return (
        <CustomDialog
            open={secretWindowOpen}
            title="Agent Secret"
            onClose={handleClose}
            submitLabel="OK"
            onSubmit={handleClose}
        >
            <Typography gutterBottom>
                Agent id
            </Typography>
            <TextField
                fullWidth
                value={createdAgentResponse?.secret || ""}
                InputProps={{
                    readOnly: true,
                    value: createdAgentResponse?.id || ""
                }}
            />
            <Typography gutterBottom>
                Please copy this secret. You will not be able to see it again:
            </Typography>
            <TextField
                fullWidth
                value={createdAgentResponse?.secret || ""}
                InputProps={{
                    readOnly: true,
                    endAdornment: (
                        <IconButton onClick={handleCopySecret}>
                            <ContentCopyIcon />
                        </IconButton>
                    ),
                }}
            />
        </CustomDialog>
    );
} 