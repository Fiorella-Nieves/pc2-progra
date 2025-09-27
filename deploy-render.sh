#!/bin/bash

echo "=== Preparando despliegue en Render ==="

# Verificar que estamos en la rama main
CURRENT_BRANCH=$(git branch --show-current)
if [ "$CURRENT_BRANCH" != "main" ]; then
    echo "ERROR: Debes estar en la rama main para desplegar"
    exit 1
fi

# Ejecutar tests básicos
echo "Ejecutando tests..."
dotnet test

if [ $? -ne 0 ]; then
    echo "ERROR: Los tests fallaron. Corrige los errores antes de desplegar."
    exit 1
fi

# Publicar la aplicación
echo "Publicando aplicación..."
dotnet publish -c Release -o ./publish

# Verificar que la publicación fue exitosa
if [ $? -ne 0 ]; then
    echo "ERROR: La publicación falló."
    exit 1
fi

echo "✅ Aplicación publicada correctamente"
echo "✅ Subiendo cambios a Render..."
echo ""
echo "Recuerda:"
echo "1. Render detectará automáticamente los cambios en main"
echo "2. Las variables de entorno deben configurarse en el dashboard de Render"
echo "3. Verifica los logs en caso de error"