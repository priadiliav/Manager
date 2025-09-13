import { HorizontalStepper } from "../stepper/HorizontalStepper";
import { CollapsibleSection } from "../wrappers/CollapsibleSection";


export const StateInformation = () => {
    return (
        <CollapsibleSection title="State Information" noCard>
            <HorizontalStepper />
        </CollapsibleSection>
    );
}