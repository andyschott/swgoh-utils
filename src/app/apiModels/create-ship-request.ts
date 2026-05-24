/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { EarnableLocation } from "./earnable-location";
import { ShipMarqueeRequest } from "./ship-marquee-request";

export interface CreateShipRequest {
    name: string;
    locations: EarnableLocation[];
    marquee: ShipMarqueeRequest | null;
}
