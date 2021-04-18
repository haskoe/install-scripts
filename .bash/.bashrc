#!/bin/bash -e

if [ -z "${DISPLAY}" ] && [ "${XDG_VTNR}" -eq 1 ]; then
  exec startx
fi

#eval $(keychain --eval id_hg id_${HOSTNAME})

npub() {
    [[ ! -f "package.json" ]] && echo "No package.json" && return 1
    npm version $1 || (echo "invalid version number" && return 1)
    npm publish --registry http://localhost:4873
}

nilr() {
    npm install $1 --registry http://localhost:4873
}

vs() {
    . ~/dev/azure-repos/cs/tools/bat/start_vscode.sh $1 $2
}

# put this in ~/.bashrc
# alias vsc='code-insiders'
#. ~/dev/azure-repos/linux/ubuntu/.bashrc
export PROMPT_COMMAND="pwd > /tmp/whereami"
export PGOPTIONS='--client-min-messages=warning' 

local_dir=~/arch-setup/.bash
heas_aliases=${local_dir}/.bash_aliases
if [ -f $heas_aliases ]; then
    . $heas_aliases
fi

NPM_PACKAGES=~/.npm-packages
NODE_PATH=~/.npm-packages/lib/node_modules
PATH=$PATH:~/.npm-packages/bin:~/dev/azure-repos/cs/tools/db/unix
MANPATH=$MANPATH:~/.npm-packages/share/man

ARM_TENANT_ID=
ARM_SUBSCRIPTION_ID=
ARM_ENVIRONMENT=public
ARM_CLIENT_ID=
ARM_CLIENT_SECRET=


if [ "$TERM_PROGRAM" != "vscode" ]
then    
    CD_TO=/tmp/whereami
    [[ -f $CD_TO ]] && cd $(cat ${CD_TO})
fi
export LC_ALL=da_DK.UTF-8
export LANG=da_DK.UTF-8

export PYTHONPATH=~/dev/azure-repos/misc/misc/fileorganizer
