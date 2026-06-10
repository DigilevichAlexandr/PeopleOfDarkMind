export default function SettingsPage() {
  return (
    <div className="max-w-lg">
      <h1 className="font-[family-name:var(--font-display)] text-3xl mb-6">Настройки</h1>
      <div className="bg-panel border border-border rounded-xl p-5 space-y-4 text-sm">
        <p className="text-muted">Автосохранение: включено (после каждого действия — в разработке)</p>
        <p className="text-muted">Ручные слоты: 3 (в разработке)</p>
      </div>
    </div>
  );
}
