/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { EarnableLocation } from "./earnable-location";
import { MarqueeRequest } from "./marquee-request";

export interface UpdateShipRequest {
    locations: EarnableLocation[] | null;
    marquee: MarqueeRequest | null;
}
