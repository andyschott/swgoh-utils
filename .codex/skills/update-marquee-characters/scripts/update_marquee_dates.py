#!/usr/bin/env python3
from __future__ import annotations

import argparse
import datetime as dt
import re
from pathlib import Path

ARRAY_PATTERN = re.compile(
    r"(?P<prefix>const\s+marquees\s*:\s*MarqueeDate\[\]\s*=\s*\[)\n(?P<body>[\s\S]*?)(?P<suffix>\n\];)",
    re.MULTILINE,
)


def parse_date(value: str) -> dt.date:
    try:
        return dt.date.fromisoformat(value)
    except ValueError as exc:
        raise argparse.ArgumentTypeError(
            f"Invalid date '{value}'. Expected YYYY-MM-DD."
        ) from exc


def add_year(value: dt.date) -> dt.date:
    try:
        return value.replace(year=value.year + 1)
    except ValueError:
        return value.replace(year=value.year + 1, day=28)


def format_date(value: dt.date) -> str:
    return value.isoformat()


def build_entries(names: list[str], introduction: dt.date, first_marquee: dt.date) -> str:
    marquee_dates = [first_marquee + dt.timedelta(days=14 * idx) for idx in range(6)]
    shipment = marquee_dates[-1] + dt.timedelta(days=22)
    farm_dates = [shipment + dt.timedelta(days=14 * idx) for idx in range(6)]
    acceleration = add_year(introduction)

    blocks: list[str] = []
    for idx, name in enumerate(names):
        escaped_name = name.replace("'", "\\'")
        block = "\n".join(
            [
                "  {",
                f"    name: '{escaped_name}',",
                f"    introduction: new Date('{format_date(introduction)}'),",
                f"    marqueeEvent: new Date('{format_date(marquee_dates[idx])}'),",
                f"    shipment: new Date('{format_date(shipment)}'),",
                f"    farm: new Date('{format_date(farm_dates[idx])}'),",
                f"    acceleration: new Date('{format_date(acceleration)}')",
                "  }",
            ]
        )
        blocks.append(block)

    return ",\n".join(blocks)


def update_file(file_path: Path, new_entries: str) -> None:
    original = file_path.read_text(encoding="utf-8")
    match = ARRAY_PATTERN.search(original)
    if not match:
        raise RuntimeError("Could not locate marquees array in target file.")

    body = match.group("body")
    body_stripped = body.lstrip("\n")
    combined_body = f"{new_entries},\n{body_stripped}"

    updated = (
        original[: match.start()]
        + match.group("prefix")
        + "\n"
        + combined_body
        + match.group("suffix")
        + original[match.end() :]
    )
    file_path.write_text(updated, encoding="utf-8")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Insert six computed marquee entries into marquee-dates.ts"
    )
    parser.add_argument(
        "--names",
        nargs=6,
        required=True,
        metavar="NAME",
        help="Exactly six character names in marquee order",
    )
    parser.add_argument(
        "--introduction",
        required=True,
        type=parse_date,
        help="Shared introduction date (YYYY-MM-DD)",
    )
    parser.add_argument(
        "--first-marquee",
        required=True,
        type=parse_date,
        help="First marquee event date (YYYY-MM-DD)",
    )
    parser.add_argument(
        "--file",
        default="/Users/aschott/Developer/swgoh-utils/src/app/marquee/marquee-dates.ts",
        type=Path,
        help="Path to marquee-dates.ts",
    )
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    target = args.file
    if not target.exists():
        raise FileNotFoundError(f"Target file not found: {target}")

    new_entries = build_entries(args.names, args.introduction, args.first_marquee)
    update_file(target, new_entries)
    print(f"Inserted 6 marquee entries into {target}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
