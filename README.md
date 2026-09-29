# review-me

A commandline tool to have an LLM of your choice review your code before a human
does.

## Usage

Simply invoke the tool to have it automatically select the most fitting review mode.
If there are any uncommitted changes, these will be reviewed - otherwise, a diff
between the current branch and the default branch (`master`/`main`) will be reviewed.

```sh
review-me
```

Review all uncommitted changes.

```sh
review-me --uncommitted
```

Review a diff from the current branch to the given *ref*; this can be a branch
or even just a commit hash.

```sh
review-me --diff main
```

Review the current state of a specific file.

```sh
review-me --file src/some/file.cs
```

### Configuration

Configuration is stored in `$XDG_CONFIG_HOME/review-me/settings.json` and can
be edited manually or by using the `review-me config` command:

Read the currently configured value for `provider`.
```sh
review-me config provider
```

Set a new value for `provider`.
```sh
review-me config provider openai;http://127.0.0.1;;
```
