import { useState } from 'react'
import { Button, Card, CardBody, Input, Spinner } from '@heroui/react'
import { AlertTriangle, Bot, Sparkles } from 'lucide-react'
import { api } from './api'

export function AnalysisPanel() {
  const [question, setQuestion] = useState('What inventory risks should we address first?')
  const [answer, setAnswer] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  const analyze = async (event: React.FormEvent) => {
    event.preventDefault()
    setError('')
    setAnswer('')
    setBusy(true)
    try {
      const result = await api.analyze(question)
      setAnswer(result.answer)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'The inventory analyst could not respond.')
    } finally {
      setBusy(false)
    }
  }

  return <><div className="page-heading compact"><div><p className="eyebrow">FLOWLINE INTELLIGENCE</p><h1>AI analysis</h1><p className="subheading">Ask a read-only analyst about the current inventory snapshot.</p></div><div className="analysis-badge"><Sparkles size={16} /> Azure OpenAI</div></div><Card className="table-card analysis-card"><CardBody><div className="analysis-intro"><span className="analysis-icon"><Bot size={23} /></span><div><h3>Inventory analyst</h3><p>Recommendations are grounded in your current stock records. The agent cannot change inventory or place orders.</p></div></div><form onSubmit={analyze} className="analysis-form"><Input label="Ask a question" value={question} onValueChange={setQuestion} isRequired /><Button type="submit" className="primary-btn" isDisabled={busy || !question.trim()}>{busy ? <Spinner size="sm" color="white" /> : <Sparkles size={16} />}Analyze inventory</Button></form>{error && <div className="error-box"><AlertTriangle size={16} />{error}</div>}{answer && <div className="analysis-answer"><h3>Analysis</h3><div>{answer.split('\n').map((line, index) => <p key={index}>{line || '\u00a0'}</p>)}</div></div>}</CardBody></Card></>
}
