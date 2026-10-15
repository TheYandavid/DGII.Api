import { useCallback, useEffect, useMemo, useState } from 'react';

const PAGE_SIZES = [5, 10, 25, 50];
const money = new Intl.NumberFormat('es-DO', { style: 'currency', currency: 'DOP', minimumFractionDigits: 2 });
const date = new Intl.DateTimeFormat('es-DO', { day: '2-digit', month: 'short', year: 'numeric' });

async function getJson(path, signal) {
  const response = await fetch(path, { signal });
  const text = await response.text();
  let body;
  try {
    body = text ? JSON.parse(text) : null;
  } catch {
    body = text;
  }
  if (!response.ok) {
    const message = typeof body === 'string'
      ? (response.status >= 500 ? 'Ocurrió un error interno al consultar la API.' : body)
      : body?.mensaje || body?.message;
    throw new Error(message || `El servidor respondió ${response.status}.`);
  }
  return body;
}

function Icon({ name, size = 20 }) {
  const common = { width: size, height: size, viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', strokeWidth: 1.7, strokeLinecap: 'round', strokeLinejoin: 'round', 'aria-hidden': true };
  const paths = {
    building: <><path d="M3 21h18M5 21V7l7-4 7 4v14M9 21v-6h6v6M9 9h.01M15 9h.01M9 12h.01M15 12h.01" /></>,
    receipt: <><path d="M5 3v18l3-2 4 2 4-2 3 2V3l-3 2-4-2-4 2-3-2Z" /><path d="M9 9h6M9 13h6" /></>,
    search: <><circle cx="10.8" cy="10.8" r="6.8" /><path d="m16 16 4.5 4.5" /></>,
    refresh: <><path d="M20 7v5h-5M4 17v-5h5" /><path d="M5.6 9a7 7 0 0 1 11.6-2L20 12M4 12l2.8 5a7 7 0 0 0 11.6-2" /></>,
    arrow: <><path d="M5 12h14M13 6l6 6-6 6" /></>,
    chevron: <path d="m9 18 6-6-6-6" />,
    check: <><path d="m5 12 4 4L19 6" /></>,
    close: <><path d="m18 6-12 12M6 6l12 12" /></>,
    chart: <><path d="M3 3v18h18" /><path d="m19 9-5 5-4-4-5 5" /></>,
  };
  return <svg {...common}>{paths[name]}</svg>;
}

function StatusBadge({ value }) {
  const active = String(value).toUpperCase() === 'ACTIVO';
  return <span className={`status-badge ${active ? 'is-active' : 'is-inactive'}`}><span />{value}</span>;
}

function EmptyState({ loading, error, columns, emptyText }) {
  const message = loading ? 'Cargando información…' : error || emptyText;
  return <tr><td colSpan={columns} className={`table-message ${error ? 'has-error' : ''}`}>
    {error ? <span className="message-icon"><Icon name="close" size={17} /></span> : loading ? <span className="spinner" /> : <span className="message-icon"><Icon name="search" size={17} /></span>}
    {message}
  </td></tr>;
}

function MetricCard({ label, value, detail, icon, tone = '' }) {
  return <article className="metric-card">
    <div className={`metric-icon ${tone}`}><Icon name={icon} size={19} /></div>
    <div className="metric-copy"><span>{label}</span><strong>{value}</strong><small>{detail}</small></div>
  </article>;
}

function Pager({ page, pageSize, total, onPageChange, onPageSizeChange }) {
  const pages = Math.max(1, Math.ceil(total / pageSize));
  return <div className="pagination">
    <span className="pagination-count">Mostrando <b>{total === 0 ? 0 : (page - 1) * pageSize + 1}–{Math.min(page * pageSize, total)}</b> de <b>{total.toLocaleString('es-DO')}</b></span>
    <div className="pagination-controls">
      <label className="page-size">Filas <select value={pageSize} onChange={(event) => onPageSizeChange(Number(event.target.value))}>{PAGE_SIZES.map((size) => <option key={size} value={size}>{size}</option>)}</select></label>
      <button className="icon-button" aria-label="Página anterior" disabled={page <= 1} onClick={() => onPageChange(page - 1)}><span className="left-chevron"><Icon name="chevron" size={17} /></span></button>
      <span className="page-indicator">{page} <span>/</span> {pages}</span>
      <button className="icon-button" aria-label="Página siguiente" disabled={page >= pages} onClick={() => onPageChange(page + 1)}><Icon name="chevron" size={17} /></button>
    </div>
  </div>;
}

function ContribuyentesView({ onOpenReceipts }) {
  const [filters, setFilters] = useState({ nombre: '', rncCedula: '', tipo: '', estatus: '' });
  const [applied, setApplied] = useState({ nombre: '', rncCedula: '', tipo: '', estatus: '' });
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [result, setResult] = useState({ data: [], total: 0 });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const filtered = Object.values(applied).some((value) => value.trim() !== '');

  useEffect(() => {
    const controller = new AbortController();
    setLoading(true);
    setError('');
    const query = new URLSearchParams();
    let path;
    if (filtered) {
      Object.entries(applied).forEach(([key, value]) => value && query.set(key, value));
      path = `/api/contribuyentes/buscar?${query}`;
    } else {
      query.set('page', page);
      query.set('pageSize', pageSize);
      path = `/api/contribuyentes?${query}`;
    }

    getJson(path, controller.signal)
      .then((response) => setResult(filtered ? { data: response, total: response.length } : { data: response.data, total: response.total }))
      .catch((reason) => {
        if (reason.name !== 'AbortError') {
          setError(reason.message || 'No se pudieron cargar los contribuyentes.');
          setResult({ data: [], total: 0 });
        }
      })
      .finally(() => { if (!controller.signal.aborted) setLoading(false); });

    return () => controller.abort();
  }, [applied, filtered, page, pageSize]);

  const totalItbis = useMemo(() => result.data.reduce((sum, taxpayer) => sum + Number(taxpayer.totalItbis || 0), 0), [result.data]);
  const activeCount = result.data.filter((taxpayer) => taxpayer.estatus?.toUpperCase() === 'ACTIVO').length;

  function updateFilter(event) {
    setFilters((current) => ({ ...current, [event.target.name]: event.target.value }));
  }

  function search(event) {
    event.preventDefault();
    setPage(1);
    setApplied({ ...filters });
  }

  function clearFilters() {
    const cleared = { nombre: '', rncCedula: '', tipo: '', estatus: '' };
    setFilters(cleared);
    setApplied(cleared);
    setPage(1);
  }

  return <>
    <section className="metrics-grid">
      <MetricCard label="Contribuyentes" value={loading ? '—' : result.total.toLocaleString('es-DO')} detail={filtered ? 'Coincidencias con filtros' : 'Registros en el sistema'} icon="building" tone="green" />
      <MetricCard label="Activos en vista" value={loading ? '—' : activeCount.toLocaleString('es-DO')} detail="Con estatus activo" icon="check" tone="mint" />
      <MetricCard label="ITBIS total" value={loading ? '—' : money.format(totalItbis)} detail={filtered ? 'De los resultados' : 'De esta página'} icon="chart" tone="gold" />
    </section>

    <section className="data-card">
      <div className="card-heading">
        <div><h2>Directorio de contribuyentes</h2><p>Consulta y filtra los registros fiscales</p></div>
        <span className="record-chip"><span className="live-dot" />{loading ? 'Actualizando' : `${result.total.toLocaleString('es-DO')} registros`}</span>
      </div>
      <form className="filter-bar" onSubmit={search}>
        <label className="filter-field search-field"><span>Nombre o razón social</span><div className="input-with-icon"><Icon name="search" size={17} /><input name="nombre" value={filters.nombre} onChange={updateFilter} placeholder="Buscar por nombre…" /></div></label>
        <label className="filter-field"><span>RNC / Cédula</span><input name="rncCedula" value={filters.rncCedula} onChange={updateFilter} placeholder="000-0000000-0" /></label>
        <label className="filter-field"><span>Tipo de contribuyente</span><select name="tipo" value={filters.tipo} onChange={updateFilter}><option value="">Todos los tipos</option><option>PERSONA FISICA</option><option>PERSONA JURIDICA</option></select></label>
        <label className="filter-field"><span>Estatus</span><select name="estatus" value={filters.estatus} onChange={updateFilter}><option value="">Todos</option><option>ACTIVO</option><option>INACTIVO</option></select></label>
        <div className="filter-actions"><button className="button button-primary" type="submit"><Icon name="search" size={16} />Buscar</button><button className="button button-quiet" type="button" onClick={clearFilters}>Limpiar</button></div>
      </form>
      <div className="table-scroll"><table className="data-table">
        <thead><tr><th>Contribuyente</th><th>RNC / Cédula</th><th>Tipo</th><th>Estatus</th><th className="align-right">Comprobantes</th><th className="align-right">ITBIS acumulado</th><th aria-label="Acciones" /></tr></thead>
        <tbody>{loading || error || result.data.length === 0
          ? <EmptyState loading={loading} error={error} columns={7} emptyText="No hay contribuyentes que coincidan con la búsqueda." />
          : result.data.map((taxpayer) => <tr key={taxpayer.rncCedula}>
            <td><div className="person-cell"><span className="avatar">{(taxpayer.nombre || '?').trim().charAt(0).toUpperCase()}</span><span className="person-name">{taxpayer.nombre}</span></div></td>
            <td className="mono">{taxpayer.rncCedula}</td><td><span className="type-label">{taxpayer.tipo === 'PERSONA FISICA' ? 'Persona física' : 'Persona jurídica'}</span></td>
            <td><StatusBadge value={taxpayer.estatus} /></td><td className="align-right tabular">{Number(taxpayer.cantidadComprobantesActivos || 0).toLocaleString('es-DO')}</td>
            <td className="align-right tabular money-cell">{money.format(taxpayer.totalItbis || 0)}</td>
            <td className="row-action"><button className="text-action" onClick={() => onOpenReceipts(taxpayer.rncCedula)} aria-label={`Ver comprobantes de ${taxpayer.nombre}`}><Icon name="arrow" size={17} /></button></td>
          </tr>)}</tbody>
      </table></div>
      {!filtered && <Pager page={page} pageSize={pageSize} total={result.total} onPageChange={setPage} onPageSizeChange={(size) => { setPageSize(size); setPage(1); }} />}
      {filtered && <div className="filter-note">La búsqueda filtrada muestra todos los resultados disponibles.</div>}
    </section>
  </>;
}

function ComprobantesView({ initialRnc }) {
  const [filters, setFilters] = useState({ rncCedula: initialRnc || '', ncf: '', estado: '' });
  const [applied, setApplied] = useState({ rncCedula: initialRnc || '', ncf: '', estado: '' });
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [result, setResult] = useState({ data: [], total: 0 });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    if (initialRnc) {
      setFilters((current) => ({ ...current, rncCedula: initialRnc }));
      setApplied((current) => ({ ...current, rncCedula: initialRnc }));
      setPage(1);
    }
  }, [initialRnc]);

  useEffect(() => {
    const controller = new AbortController();
    setLoading(true);
    setError('');
    const query = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
    Object.entries(applied).forEach(([key, value]) => value && query.set(key, value));
    getJson(`/api/comprobantes?${query}`, controller.signal)
      .then((response) => setResult({ data: response.data, total: response.total }))
      .catch((reason) => {
        if (reason.name !== 'AbortError') {
          setError(reason.message || 'No se pudieron cargar los comprobantes.');
          setResult({ data: [], total: 0 });
        }
      })
      .finally(() => { if (!controller.signal.aborted) setLoading(false); });
    return () => controller.abort();
  }, [applied, page, pageSize]);

  const totalItbis = useMemo(() => result.data.reduce((sum, receipt) => sum + Number(receipt.itbis18 || 0), 0), [result.data]);
  const activeCount = result.data.filter((receipt) => receipt.estado?.toUpperCase() === 'ACTIVO').length;

  function updateFilter(event) {
    setFilters((current) => ({ ...current, [event.target.name]: event.target.value }));
  }

  function search(event) {
    event.preventDefault();
    setPage(1);
    setApplied({ ...filters });
  }

  function clearFilters() {
    const cleared = { rncCedula: '', ncf: '', estado: '' };
    setFilters(cleared);
    setApplied(cleared);
    setPage(1);
  }

  return <>
    <section className="metrics-grid">
      <MetricCard label="Comprobantes" value={loading ? '—' : result.total.toLocaleString('es-DO')} detail="Coinciden con la consulta" icon="receipt" tone="green" />
      <MetricCard label="Activos en vista" value={loading ? '—' : activeCount.toLocaleString('es-DO')} detail="Documentos vigentes" icon="check" tone="mint" />
      <MetricCard label="ITBIS de la página" value={loading ? '—' : money.format(totalItbis)} detail="Suma de los registros visibles" icon="chart" tone="gold" />
    </section>

    <section className="data-card">
      <div className="card-heading">
        <div><h2>Comprobantes fiscales</h2><p>Revisa NCF, montos y estado de emisión</p></div>
        <span className="record-chip"><span className="live-dot" />{loading ? 'Actualizando' : `${result.total.toLocaleString('es-DO')} registros`}</span>
      </div>
      <form className="filter-bar receipt-filters" onSubmit={search}>
        <label className="filter-field"><span>RNC / Cédula</span><input name="rncCedula" value={filters.rncCedula} onChange={updateFilter} placeholder="Identificación fiscal" /></label>
        <label className="filter-field"><span>Número de comprobante (NCF)</span><input name="ncf" value={filters.ncf} onChange={updateFilter} placeholder="Ej. B0100000001" /></label>
        <label className="filter-field"><span>Estado</span><select name="estado" value={filters.estado} onChange={updateFilter}><option value="">Todos los estados</option><option>ACTIVO</option><option>INACTIVO</option></select></label>
        <div className="filter-actions"><button className="button button-primary" type="submit"><Icon name="search" size={16} />Buscar</button><button className="button button-quiet" type="button" onClick={clearFilters}>Limpiar</button></div>
      </form>
      <div className="table-scroll"><table className="data-table receipts-table">
        <thead><tr><th>NCF</th><th>RNC / Cédula</th><th>Fecha de emisión</th><th className="align-right">Monto</th><th className="align-right">ITBIS 18%</th><th>Estado</th></tr></thead>
        <tbody>{loading || error || result.data.length === 0
          ? <EmptyState loading={loading} error={error} columns={6} emptyText="No hay comprobantes que coincidan con la búsqueda." />
          : result.data.map((receipt) => <tr key={receipt.ncf}>
            <td className="mono ncf-cell">{receipt.ncf}</td><td className="mono">{receipt.rncCedula}</td>
            <td className="date-cell">{date.format(new Date(receipt.fechaEmision))}</td>
            <td className="align-right tabular money-cell">{money.format(receipt.monto || 0)}</td>
            <td className="align-right tabular money-cell">{money.format(receipt.itbis18 || 0)}</td><td><StatusBadge value={receipt.estado} /></td>
          </tr>)}</tbody>
      </table></div>
      <Pager page={page} pageSize={pageSize} total={result.total} onPageChange={setPage} onPageSizeChange={(size) => { setPageSize(size); setPage(1); }} />
    </section>
  </>;
}

export default function App() {
  const [activeView, setActiveView] = useState('taxpayers');
  const [apiOnline, setApiOnline] = useState(null);
  const [selectedRnc, setSelectedRnc] = useState('');

  const checkApi = useCallback(() => {
    const controller = new AbortController();
    getJson('/api/health', controller.signal)
      .then(() => setApiOnline(true))
      .catch(() => setApiOnline(false));
    return () => controller.abort();
  }, []);

  useEffect(() => checkApi(), [checkApi]);

  function openReceipts(rncCedula) {
    setSelectedRnc(rncCedula);
    setActiveView('receipts');
  }

  return <div className="app-shell">
    <aside className="sidebar">
      <a className="brand" href="#inicio" aria-label="DGII, inicio" onClick={(event) => { event.preventDefault(); setActiveView('taxpayers'); }}>
        <span className="brand-mark"><Icon name="building" size={22} /></span>
        <span className="brand-text"><strong>DGII</strong><small>Portal de consulta</small></span>
      </a>
      <div className="side-label">GESTIÓN FISCAL</div>
      <nav className="side-nav" aria-label="Navegación principal">
        <button className={activeView === 'taxpayers' ? 'nav-item selected' : 'nav-item'} onClick={() => { setSelectedRnc(''); setActiveView('taxpayers'); }}><Icon name="building" size={19} /><span>Contribuyentes</span><Icon name="chevron" size={15} /></button>
        <button className={activeView === 'receipts' ? 'nav-item selected' : 'nav-item'} onClick={() => { setSelectedRnc(''); setActiveView('receipts'); }}><Icon name="receipt" size={19} /><span>Comprobantes</span><Icon name="chevron" size={15} /></button>
      </nav>
      <div className="sidebar-bottom">
        <div className="connection-card"><span className={`connection-dot ${apiOnline === true ? 'online' : apiOnline === false ? 'offline' : ''}`} /><span><b>{apiOnline === true ? 'API conectada' : apiOnline === false ? 'API no disponible' : 'Conectando…'}</b><small>Estado del servicio</small></span><button className="connection-refresh" onClick={checkApi} aria-label="Comprobar conexión"><Icon name="refresh" size={15} /></button></div>
        <div className="sidebar-footer">DIRECCIÓN GENERAL<br />DE IMPUESTOS INTERNOS</div>
      </div>
    </aside>

    <main className="main-content">
      <header className="topbar"><div className="breadcrumb">Panel de consulta <Icon name="chevron" size={14} /><span>{activeView === 'taxpayers' ? 'Contribuyentes' : 'Comprobantes fiscales'}</span></div><div className="topbar-meta"><span className="environment-pill"><span />Sistema de consulta</span><span className="user-avatar" aria-label="Usuario">DG</span></div></header>
      <div className="page-container">
        <section className="page-intro"><div><div className="eyebrow">ADMINISTRACIÓN TRIBUTARIA <span /></div><h1>{activeView === 'taxpayers' ? 'Contribuyentes' : 'Comprobantes fiscales'}</h1><p>{activeView === 'taxpayers' ? 'Consulta la información fiscal y el ITBIS reportado por contribuyente.' : 'Explora y filtra los documentos fiscales registrados en el sistema.'}</p></div><div className="intro-date"><span>HOY</span><strong>{date.format(new Date())}</strong></div></section>
        {activeView === 'taxpayers' ? <ContribuyentesView onOpenReceipts={openReceipts} /> : <ComprobantesView key={selectedRnc || 'all'} initialRnc={selectedRnc} />}
        <footer className="page-footer"><span>DGII · Sistema de consulta fiscal</span><span>Información sujeta a actualización</span></footer>
      </div>
    </main>
  </div>;
}
