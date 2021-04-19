export ZSH="$HOME/.sheldon/repos/github.com/ohmyzsh/ohmyzsh"

eval "$(sheldon source)"

# To customize prompt, run `p10k configure` or edit ${ZDOTDIR}/.p10k.zsh.
[[ ! -f ${ZDOTDIR}/.p10k.zsh ]] || source ${ZDOTDIR}/.p10k.zsh
