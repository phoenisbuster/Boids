#!/bin/bash

# Replace __TOKEN_PLACEHOLDER__ with actual token
sed -i '' "s|%BOT_PRIVATE_TOKEN%|$BOT_PRIVATE_TOKEN|g" ./Assets/NuGet.config

# Add pre-commit hook to Git if not already present
if [ ! -f ".git/hooks/pre-commit" ]; then
  cp ./carem-template-sh/pre-commit .git/hooks/pre-commit
  chmod +x .git/hooks/pre-commit
fi