/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { EarnableLocation } from "./earnable-location";
import { CharacterMarqueeRequest } from "./character-marquee-request";

export interface UpdateCharacterRequest {
    locations: EarnableLocation[] | null;
    isAccelerated: boolean | null;
    marquee: CharacterMarqueeRequest | null;
}
