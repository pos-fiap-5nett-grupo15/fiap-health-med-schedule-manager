rm deployment2.yaml
cp deployment.yaml deployment2.yaml


sed -i 's/PLACEHOLDER_NAMESPACE/hk/g' deployment2.yaml
sed -i 's/PLACEHOLDER_CONTAINER_REGISTRY/fiapacrhackathon.azurecr.io/g' deployment2.yaml
sed -i 's/PLACEHOLDER_IMAGE_REPOSITORY/fiap-health-med-schedule-manager-api/g' deployment2.yaml
sed -i 's/PLACEHOLDER_TAG/latest/g' deployment2.yaml

cat deployment2.yaml
