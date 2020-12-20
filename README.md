# DumpSym2IdaPython

Converts DUMPSYM output to an IDAPython script for easy symbol renaming

## Usage

This small utility parses the output of SN-Systems' `DUMPSYM.EXE` found in the official PSX SDK.

It will extract all `LABEL` tags and generate an IDAPython for batch renaming symbols in IDA.

Example:

`DumpSym2IdaPython --source "NTSC.SYM" --target "NTSC.py" --prefix "__" --sorted`

Using `--prefix` and `--sorted` are recommended so that you can:

- easily differentatiate changes brought by you VS initial IDA analysis
- process them in file order for easily fixing tail byte errors

Your mileage may vary, some games will produce good results, some won't, e.g.:

- `PSX.SYM` in Twisted Metal (NTSC-J) seems to be correct
- `NTSC.SYM` in Wipeout XL Beta (NTSC-U/C) is definitely not right

In short, some SYM files you can find just appear to be a leftover for a previous build. 😭
