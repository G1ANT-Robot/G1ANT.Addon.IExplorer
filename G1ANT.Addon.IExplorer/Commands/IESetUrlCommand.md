# ie.seturl

## Syntax

```G1ANT
ie.seturl url ⟦text⟧ nowait ⟦bool⟧
```

## Description

This command navigates Internet Explorer to a specified URL address.

| Argument | Type | Required | Default Value | Description |
| -------- | ---- | -------- | ------------- | ----------- |
|`url`| [text](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/TextStructure.md) | no |  | URL address to navigate to |
|`nowait`| [bool](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/BooleanStructure.md) | no | false | If set to `true`, the command will not wait until the page is loaded |
|`result`| [variable](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/VariableStructure.md) | no | `♥result` | Name of a variable where the command's result (an attached Internet Explorer instance ID) will be stored |
| `if`           | [bool](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/BooleanStructure.md) | no       | true                                                        | Executes the command only if a specified condition is true   |
| `timeout`      | [timespan](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/TimeSpanStructure.md) | no       | [♥timeoutie](https://manual.g1ant.com/link/G1ANT.Addon.IExplorer/G1ANT.Addon.IExplorer/Variables/TimeoutIEVariable.md) | Specifies time in milliseconds for G1ANT.Robot to wait for the command to be executed |
| `errorcall`    | [procedure](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/ProcedureStructure.md) | no       |                                                             | Name of a procedure to call when the command throws an exception or when a given `timeout` expires |
| `errorjump`    | [label](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/LabelStructure.md) | no       |                                                             | Name of the label to jump to when the command throws an exception or when a given `timeout` expires |
| `errormessage` | [text](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/TextStructure.md) | no       |                                                             | A message that will be shown in case the command throws an exception or when a given `timeout` expires, and no `errorjump` argument is specified |
| `errorresult`  | [variable](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/VariableStructure.md) | no       |                                                             | Name of a variable that will store the returned exception. The variable will be of [error](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/ErrorStructure.md) structure  |

For more information about `if`, `timeout`, `errorcall`, `errorjump`, `errormessage` and `errorresult` arguments, see [Common Arguments](https://manual.g1ant.com/link/G1ANT.Manual/appendices/common-arguments.md) page.

## Example

In this simple script the robot opens Google in Internet Explorer, focuses on the browser window, then navigates to G1ANT website:

```G1ANT
ie.open google.com
window ‴✱internet explorer✱‴
ie.seturl g1ant.com
```

> **Note:** Before using this command, the [`ie.attach`](https://manual.g1ant.com/link/G1ANT.Addon.IExplorer/G1ANT.Addon.IExplorer/Commands/IEAttachCommand.md) or the [`ie.open`](https://manual.g1ant.com/link/G1ANT.Addon.IExplorer/G1ANT.Addon.IExplorer/Commands/IEOpenCommand.md) command has to be executed.
