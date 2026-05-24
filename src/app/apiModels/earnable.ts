/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { EarnableLocation } from "./earnable-location";
import { Marquee } from "./marquee";

export interface Earnable {
    id: string;
    name: string;
    locations: EarnableLocation[];
    marquee: Marquee | null;
}
