# review-me

A commandline tool to have an LLM of your choice review your code before a human
does.

## Installation

The tool is available via NuGet: `dotnet tool install --global ReviewMe`. After installation,
you should configure the connection string to the LLM.

```
review-me config ConnectionString Endpoint=$URL;Provider=OpenAI;Model=$MODEL
```

I'm working on a locally running [llama.cpp](https://github.com/ggml-org/llama.cpp) server running
[unsloth/Qwen3.6](https://huggingface.co/unsloth/Qwen3.6-35B-A3B-GGUF); it's giving me okay results.
It should work with every OpenAI-API compatible provider, though. Sends your code to it, though -
the tool is built with a local model in mind.

## Usage

Simply invoke the tool to have it automatically select the most fitting review mode.
If there are any uncommitted changes, these will be reviewed - otherwise, a diff
between the current branch and the default branch (`master`/`main`) will be reviewed.
If the current branch is already the default branch, the local `HEAD` is compared against
the remote `HEAD`.

```sh
review-me
```

Review all uncommitted changes:
```sh
review-me --uncommitted
```

Review a diff from the current branch to the given *ref*; this can be a branch
or even just a commit hash:
```sh
review-me --diff main
```

Review the current state of a specific file:
```sh
review-me --file src/some/file.cs
```

### Configuration

Configuration is stored in `$XDG_CONFIG_HOME/review-me/settings.json` and can
be edited manually or by using the `review-me config` command:

Read the currently configured value for `ConnectionString`:
```sh
review-me config ConnectionString
```

Set a new value for `ConnectionString`:
```sh
review-me config ConnectionString Endpoint=http://127.0.0.1:8080/v1;Provider=OpenAI;Model=Qwen3.6
```
