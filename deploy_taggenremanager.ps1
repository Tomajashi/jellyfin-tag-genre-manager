param(
    [string]$RemoteUser = "root",
    [string]$RemoteHost = "192.168.0.38",
    [string]$RemotePluginPath = "/mnt/tank/jellyfin/config/plugins/TagGenreManager",
    [string]$LocalNet9Path = "D:\\projekt\\jellyfin-tag-genre-manager\\Jellyfin.Plugin.TagGenreManager\\bin\\Release\\net9.0"
)

if (-not (Test-Path $LocalNet9Path)) { Write-Error "Local build path not found: $LocalNet9Path"; exit 1 }

Write-Host "Copying net9.0 to ${RemoteUser}@${RemoteHost}:${RemotePluginPath}/net9.0 ..."
& scp -r "$LocalNet9Path" "${RemoteUser}@${RemoteHost}:${RemotePluginPath}/net9.0"
if ($LASTEXITCODE -ne 0) { Write-Error "scp failed (exit $LASTEXITCODE)"; exit 1 }

$remoteCmd = @'
set -e
PLUGINDIR="/mnt/tank/jellyfin/config/plugins/TagGenreManager"
mkdir -p "$PLUGINDIR/net9.0"
chown -R 1000:1000 "$PLUGINDIR" || true
chmod -R u+rwX,go+rX "$PLUGINDIR" || true
if [ -d "$PLUGINDIR/TagGenreManager" ]; then
  mkdir -p "$PLUGINDIR/net9.0"
  mv "$PLUGINDIR/TagGenreManager/"* "$PLUGINDIR/net9.0/" 2>/dev/null || true
  rmdir "$PLUGINDIR/TagGenreManager" 2>/dev/null || true
  chown -R 1000:1000 "$PLUGINDIR/net9.0" || true
fi
if [ -f "$PLUGINDIR/net9.0/Jellyfin.Plugin.TagGenreManager.dll" ]; then
  echo "DLL present"
else
  echo "DLL NOT found"
fi
if command -v systemctl >/dev/null 2>&1; then
  systemctl restart jellyfin || echo "systemctl restart failed or service not present"
else
  echo "No systemd detected; if Jellyfin runs in Docker, restart the container manually"
fi
ls -la "$PLUGINDIR"
tail -n 200 /config/log/jellyfin/server-*.log 2>/dev/null || tail -n 200 /config/log/jellyfin/server.log 2>/dev/null || true
'@

# Normalize line endings to LF and pipe the script to remote shell to avoid CRLF/zsh parsing issues
$remoteScript = $remoteCmd -replace "`r`n", "`n"
Write-Host "Running remote remediation commands (piping script to remote shell)..."
$remoteScript | & ssh "$RemoteUser@$RemoteHost" 'sh -s'