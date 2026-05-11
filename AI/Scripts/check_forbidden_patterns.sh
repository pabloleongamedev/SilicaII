#!/usr/bin/env bash

ROOT="Assets/Project/Scripts"

echo "Checking forbidden patterns in $ROOT"

grep -R "FindObjectOfType" "$ROOT" || true
grep -R "GameObject.Find" "$ROOT" || true
grep -R "Resources.LoadAll" "$ROOT" || true
grep -R "SendMessage" "$ROOT" || true
grep -R "BroadcastMessage" "$ROOT" || true
