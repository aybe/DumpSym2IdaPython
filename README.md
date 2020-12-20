# DumpSym2IdaPython

Converts DUMPSYM output to an IDAPython script for easy symbol renaming

## Usage

This small utility parses the output of SN-Systems' `DUMPSYM.EXE` found in the official PSX SDK.

It will extract all `LABEL` tags and generate an IDAPython script for batch rename of symbols in IDA.

Example:

`DumpSym2IdaPython --source "NTSC.SYM" --target "NTSC.py" --prefix "__" --sorted`

Using `--prefix` and `--sorted` are recommended so that you can:

- easily differentatiate changes brought by you against initial IDA analysis
- process them in file order for easily fixing tail byte errors because there will be some

Your mileage may vary however, for some games it works well, for some it doesn't at all:

Good results:

- Destruction Derby (Japan) `DEMOLISH.SYM`
- Hi-Octane (Europe) `MAIN.SYM`
- Twisted Metal (NTSC-J) `PSX.SYM`

Bad results:

- Wipeout XL Beta (NTSC-U/C) `NTSC.SYM`

Even though this tool doesn't implement all tags, a proper SYM file should tag 99% of the methods in IDA.

If it's not the case then for sure you have a SYM file for a different build, don't understimate that fact.

In the case of Wipeout XL, the SYM file is five days newer than the EXE, that doesn't makes sense at all.

## Links

List of games with debug symbols:

https://www.retroreversing.com/ps1-debug-symbols

Reverse-engineering of dumpsym by lab313ru:

https://github.com/lab313ru/dumpsym_src

Visual Studio solution + Windows build of dumpsym:

https://github.com/aybe/dumpsym_src
