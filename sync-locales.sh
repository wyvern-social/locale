#!/usr/bin/env bash
set -euo pipefail

REMOTE_NAME=locales
REMOTE_URL=https://github.com/wyvern-social/locale.git
BRANCH=main
PREFIX=Resources/Locales

if ! git remote | grep -q "^$REMOTE_NAME$"; then
  echo "Adding remote $REMOTE_NAME..."
  git remote add $REMOTE_NAME $REMOTE_URL
fi

case "${1:-}" in
  add)
    echo "Adding $REMOTE_NAME/$BRANCH into $PREFIX..."
    git fetch $REMOTE_NAME
    git subtree add --prefix=$PREFIX $REMOTE_NAME $BRANCH --squash
    ;;
  pull)
    echo "Pulling updates from $REMOTE_NAME/$BRANCH into $PREFIX..."
    git fetch $REMOTE_NAME
    git subtree pull --prefix=$PREFIX $REMOTE_NAME $BRANCH --squash
    ;;
  push)
    echo "Pushing changes in $PREFIX back to $REMOTE_NAME/$BRANCH..."
    git subtree push --prefix=$PREFIX $REMOTE_NAME $BRANCH
    ;;
  *)
    echo "Usage: $0 {add|pull|push}"
    echo "  add   – first-time import of locales repo into $PREFIX/"
    echo "  pull  – update locales from remote"
    echo "  push  – send changes back to remote"
    exit 1
    ;;
esac
