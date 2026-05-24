/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { EarnableLocation } from "./earnable-location";
import { CharacterMarqueeRequest } from "./character-marquee-request";

export interface CreateCharacterRequest {
    name: string;
    locations: EarnableLocation[];
    isAccelerated: boolean;
    marquee: CharacterMarqueeRequest | null;
}
